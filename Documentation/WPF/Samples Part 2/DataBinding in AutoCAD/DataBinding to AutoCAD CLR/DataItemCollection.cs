using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Diagnostics;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace DataBindingToAcadCLR
{
  delegate ObjectId GetCollectionForActiveDocument();

  class DataItemCollection : IEnumerable<DataItem>, INotifyCollectionChanged
  {

    #region INotifyCollectionChanged Members

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    #endregion

    GetCollectionForActiveDocument m_callback;
    public DataItemCollection(GetCollectionForActiveDocument callback)
    {
      m_callback = callback;
      Application.Idle += new EventHandler(Application_Idle);
      Application.DocumentManager.DocumentActivated += new DocumentCollectionEventHandler(DocumentManager_DocumentActivated);
      Application.DocumentManager.DocumentDestroyed += new DocumentDestroyedEventHandler(DocumentManager_DocumentDestroyed);
      Reset();
    }

    void DocumentManager_DocumentDestroyed(object sender, DocumentDestroyedEventArgs e)
    {
      m_list.Clear();
      m_containerId = ObjectId.Null;
      modified = true;
    }
    void DocumentManager_DocumentActivated(object sender, DocumentCollectionEventArgs e)
    {
      modified = true;
    }
    bool modified = false;
    void Application_Idle(object sender, EventArgs e)
    {
      if (modified)
      {
        Reset();
        CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      }
      modified = false;
    }
    List<DataItem> m_list = new List<DataItem>();
    ObjectId m_containerId;
    void Reset()
    {
      //detach any previous stuff
      if (!m_containerId.IsNull)
      {
        Attach(false);
        m_containerId = ObjectId.Null;
      }

      if (Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.Count > 0)
      {
        //attach to active doc
        m_containerId = m_callback();
        Attach(true);
      }
    }
    void Attach(bool attach)
    {
      Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.GetDocument(m_containerId.Database);
      if (doc == null)
        return; //has been destoyed already
      using (DocumentLock l = doc.LockDocument())
      {
        using (Transaction transaction = m_containerId.Database.TransactionManager.StartTransaction())
        {
          DBObject container = m_containerId.GetObject(OpenMode.ForRead);
          if (attach)
          {
            Debug.Assert(m_list.Count == 0);
            container.ObjectClosed += new ObjectClosedEventHandler(lt_ObjectClosed);
            foreach (ObjectId id in (System.Collections.IEnumerable)container)
              m_list.Add(new DataItem(id));
          }
          else
          {
            foreach (DataItem item in m_list)
              item.Dispose();
            m_list.Clear();
            container.ObjectClosed -= new ObjectClosedEventHandler(lt_ObjectClosed);
          }
          transaction.Commit();
        }
      }
    }

    void lt_ObjectClosed(object sender, ObjectClosedEventArgs e)
    {
      modified = true;
    }


    #region IEnumerable<LayerItem> Members

    public IEnumerator<DataItem> GetEnumerator()
    {
      return m_list.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return m_list.GetEnumerator();
    }

    #endregion
  }
}
