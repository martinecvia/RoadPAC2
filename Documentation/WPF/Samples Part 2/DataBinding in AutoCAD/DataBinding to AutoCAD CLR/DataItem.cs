using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;

namespace DataBindingToAcadCLR
{
  class DBPropertyDescriptor : PropertyDescriptor
  {
    PropertyDescriptor m_prop;

    public DBPropertyDescriptor(PropertyDescriptor prop) : base(prop.Name, null)
    {
      m_prop = prop;
    }
    public override bool CanResetValue(object component)
    {
      return m_prop.CanResetValue(component);
    }
    public override Type ComponentType
    {
      get 
      {
        return m_prop.ComponentType;
      }
    }
    public override bool IsReadOnly
    {
      get
      {
        return m_prop.IsReadOnly;
      }
    }

    public override Type PropertyType
    {
      get
      {
        return m_prop.PropertyType;
      }
    }
    public override void ResetValue(object component)
    {
      m_prop.ResetValue(component);
    }

    public override bool ShouldSerializeValue(object component)
    {
      return m_prop.ShouldSerializeValue(component);
    }

    public override object GetValue(object component)
    {
      DataItem item = (DataItem)component;
      ObjectId id = item.ObjectId;
      Database db = id.Database;
      Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.GetDocument(db);
      using (DocumentLock l = doc.LockDocument())
      {
        using (Transaction t = id.Database.TransactionManager.StartTransaction())
        {
          DBObject obj = id.GetObject(OpenMode.ForRead);
          return m_prop.GetValue(obj);
        }
      }
    }

    public override void SetValue(object component, object value)
    {
      DataItem item = (DataItem)component;
      ObjectId id = item.ObjectId;
      Database db = id.Database;
      Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.GetDocument(db);
      using (DocumentLock l = doc.LockDocument())
      {
        using (Transaction t = id.Database.TransactionManager.StartTransaction())
        {
          DBObject obj = id.GetObject(OpenMode.ForWrite);
          m_prop.SetValue(obj, value);
          t.Commit();
        }
      }
    }
  }


  class DataItem : INotifyPropertyChanged, IDisposable, ICustomTypeDescriptor
  {
    public DataItem(ObjectId id)
    {
      m_id = id;
      Attach(true);
    }

    public ObjectId ObjectId
    {
      get { return m_id; }
    }
    void Attach(bool attach)
    {
      Database db = m_id.Database;
      Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.GetDocument(db);
      if (doc == null)
        return;
      using (DocumentLock l = doc.LockDocument())
      {
        using (Transaction t = m_id.Database.TransactionManager.StartTransaction())
        {
          DBObject ltr = m_id.GetObject(OpenMode.ForRead);
          if (attach)
            ltr.ObjectClosed += new ObjectClosedEventHandler(ltr_ObjectClosed);
          else
            ltr.ObjectClosed -= new ObjectClosedEventHandler(ltr_ObjectClosed);
          t.Commit();
        }
      }
      if (attach)
        Autodesk.AutoCAD.ApplicationServices.Application.Idle += new EventHandler(Application_Idle);
      else
        Autodesk.AutoCAD.ApplicationServices.Application.Idle -= new EventHandler(Application_Idle);
    }
    #region IDisposable Members

    public void Dispose()
    {
      if (!m_id.IsNull)
      {
        Attach(false);
        m_id = ObjectId.Null;
      }
    }

    #endregion
    void Application_Idle(object sender, EventArgs e)
    {
      if (modified)
        PropertyChanged(this, new PropertyChangedEventArgs(null));
      modified = false;

    }
    bool modified = false;
    void ltr_ObjectClosed(object sender, ObjectClosedEventArgs e)
    {
      modified = true;
    }
    ObjectId m_id;

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion



    #region ICustomTypeDescriptor Members

    public System.ComponentModel.AttributeCollection GetAttributes()
    {
      throw new NotImplementedException();
    }

    public string GetClassName()
    {
      throw new NotImplementedException();
    }

    public string GetComponentName()
    {
      throw new NotImplementedException();
    }

    public TypeConverter GetConverter()
    {
      throw new NotImplementedException();
    }

    public EventDescriptor GetDefaultEvent()
    {
      throw new NotImplementedException();
    }

    public PropertyDescriptor GetDefaultProperty()
    {
      throw new NotImplementedException();
    }

    public object GetEditor(Type editorBaseType)
    {
      throw new NotImplementedException();
    }

    public EventDescriptorCollection GetEvents(Attribute[] attributes)
    {
      throw new NotImplementedException();
    }

    public EventDescriptorCollection GetEvents()
    {
      throw new NotImplementedException();
    }

    public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
    {
      throw new NotImplementedException();
    }
    PropertyDescriptorCollection m_props;
    public PropertyDescriptorCollection GetProperties()
    {
      if (m_props == null)
      {
        PropertyDescriptorCollection props;
        Database db = m_id.Database;
        Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.GetDocument(db);
        using (DocumentLock l = doc.LockDocument())
        {
          using (Transaction t = db.TransactionManager.StartTransaction())
          {
            DBObject obj = m_id.GetObject(OpenMode.ForRead);
            props = TypeDescriptor.GetProperties(obj);
            t.Commit();
          }
        }
        DBPropertyDescriptor[] propWrappers = new DBPropertyDescriptor[props.Count];
        for (int i = 0; i < props.Count; i++)
          propWrappers[i] = new DBPropertyDescriptor(props[i]);
        m_props = new PropertyDescriptorCollection(propWrappers);
      }
      return m_props;
    }

    public object GetPropertyOwner(PropertyDescriptor pd)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
