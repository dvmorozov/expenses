CREATE ROLE [DataSync_admin]
    AUTHORIZATION [dbo];


GO
EXECUTE sp_addextendedproperty @name = N'MS_name', @value = N'DataSync', @level0type = N'USER', @level0name = N'DataSync_admin';

